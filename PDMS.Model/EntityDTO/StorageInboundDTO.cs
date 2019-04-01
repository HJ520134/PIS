using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageInboundDTO: EntityDTOBase
    {
        public int Storage_Inbound_UID { get; set; }
        public int Storage_Inbound_Type_UID { get; set; }
        public string Storage_Inbound_ID { get; set; }
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public Nullable<decimal> Inbound_Price { get; set; }
        public Nullable<decimal> Current_POPrice { get; set; }
        public int PartType_UID { get; set; }
        public string PU_NO { get; set; }
        public decimal PU_Qty { get; set; }
        public string Issue_NO { get; set; }
        public decimal Be_Check_Qty { get; set; }
        public decimal OK_Qty { get; set; }
        public decimal NG_Qty { get; set; }
        public int Status_UID { get; set; }
        public string Desc { get; set; }
        public int Applicant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Applicant_Date { get; set; }
        public int Approver_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Approver_Date { get; set; }

        public string Funplant { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Classification { get; set; }
        public decimal Inbound_Qty { get; set; }
        public string Warehouse_ID { get; set; }
        public string Name_ZH { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string PartType { get; set; }
        public string Status { get; set; }
        public string BG { get; set; }
        public string Accepter { get; set; }
        public string Modifieder { get; set; }
        public string Storage_Inbound_Type { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Sum_PuQty { get; set; }
        public int BG_Organization_UID { get; set; }
        public int Plant_UID { get; set; }
        public string CoupaPO_ID { get; set; }
        public decimal Material_LastQty { get; set; }
        public decimal Material_Price { get; set; }
    }
}
