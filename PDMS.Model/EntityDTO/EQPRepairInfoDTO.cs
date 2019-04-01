using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;
namespace PDMS.Model.EntityDTO
{
    public class EQPRepairInfoDTO: EntityDTOBase
    {
 
            public string Material_Uid { get; set; }
        public string Material_Name { get; set; }
        public string Material_Id { get; set; }
        public string Material_Types { get; set; }

        public int Repair_Uid { get; set; }
        public int EQP_Uid { get; set; }
        public string Repair_id { get; set; }
        public string Status { get; set; }
        public string Error_Types { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Error_Time { get; set; }
        public string Error_Level { get; set; }
        public string Contact { get; set; }
        public string Contact_tel { get; set; }
        public string Reason_Types { get; set; }
        public string Repair_Method { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Repair_BeginTime { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Repair_EndTime { get; set; }
        public string Repair_Result { get; set; }
        public string Labor_List { get; set; }
        public string Update_Part { get; set; }
        public Nullable<decimal> Labor_Time { get; set; }
        public Nullable<decimal> All_RepairCost { get; set; }
        public Double TotalTime { get; set; }
        public string OP_TYPES { get; set; }
        public int Project_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string Process { get; set; }
        public string Repair_Reason { get; set; }
        public string Reason_Analysis { get; set; }
        public string allcost { get; set; }
        public string Repair_Remark { get; set; }
        public string Equipment { get; set; }
        public string Mfg_Serial_Num { get; set; }
        public int? EQPUSER_Uid { get; set; }
        public string Asset { get; set; }
        public string Class_Desc { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Modified_Date { get; set; }
        public DateTime End_Date_From { get; set; }
        public DateTime End_Date_To { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Apply_Time { get; set; }    
        public int Organization_UID { get; set; }

        public string Repair_Date { get; set; }
        public string Repair_Time { get; set; }
        public string EQP_Location { get; set; }
        public string FunPlant { get; set; }
        public string Project_Name { get; set; }
        public string EQP_Plant_No { get; set; }
        public int FunPlant_OrganizationUID { get; set; }
        /// <summary>
        /// 提报人员
        /// </summary>
        public string Mentioner { get; set; }
        //成本中心
        public int? CostCtr_UID { get; set; }
        public string CostCtr_ID { get; set; }
        public string CostCtr_Description { get; set; }
        //更換料件信息
        public List<MeterialUpdateInfoWithWarehouseDTO> listMeterialUpdateInfoWithWarehouse { get; set; }
        //顉用人(出庫單使用)
        public int? EQPUser_Uid { get; set; }
    }


    public class EQPRepairInfoSearchDTO : EntityDTOBase
    {
        public string Material_Name { get; set; }
        public string Material_Id { get; set; }
        public string Material_Types { get; set; }
        public string Material_Uid { get; set; }

        public int Repair_Uid { get; set; }
        public int Plant_OrganizationUID { get; set; }
        public int EQP_Uid { get; set; }
        public string Repair_id { get; set; }
        public string Status { get; set; }
        public string Error_Types { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Error_Time { get; set; }
        public string Error_Level { get; set; }
        public string Contact { get; set; }
        public string Contact_tel { get; set; }
        public string Reason_Types { get; set; }
        public string Repair_Method { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Repair_BeginTime { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Repair_EndTime { get; set; }
        public string Repair_Result { get; set; }
        public string Labor_List { get; set; }
        public string Update_Part { get; set; }
        public Nullable<decimal> Labor_Time { get; set; }
        public Nullable<decimal> All_RepairCost { get; set; }
        public Double TotalTime { get; set; }
        public string OP_TYPES { get; set; }
        public int Project_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string Process { get; set; }
        public string Repair_Reason { get; set; }
        public string Reason_Analysis { get; set; }
        public string allcost { get; set; }
        public string Repair_Remark { get; set; }
        public string Equipment { get; set; }
        public string Mfg_Serial_Num { get; set; }
        public int? EQPUSER_Uid { get; set; }
        public string Asset { get; set; }
        public string Class_Desc { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Modified_Date { get; set; }
        public DateTime End_Date_From { get; set; }
        public DateTime End_Date_To { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Apply_Time { get; set; }
        public int Organization_UID { get; set; }

        public string Repair_Date { get; set; }
        public string Repair_Time { get; set; }
        public string EQP_Location { get; set; }
        public string FunPlant { get; set; }
        public string Project_Name { get; set; }
        public string EQP_Plant_No { get; set; }
        public int FunPlant_OrganizationUID { get; set; }
        /// <summary>
        /// 提报人员
        /// </summary>
        public string Mentioner { get; set; }
        //成本中心
        public string CostCtr { get; set; }
        public int CostCtr_UID { get; set; }
    }
}
