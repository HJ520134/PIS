using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageCheckDTO: EntityDTOBase
    {
        public int Storage_Check_UID { get; set; }
        public string Storage_Check_ID { get; set; }
        public int Material_Uid { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public Nullable<decimal> Check_Price { get; set; }
        public int PartType_UID { get; set; }
        public string PartType { get; set; }
        public decimal Old_Inventory_Qty { get; set; }
        public decimal Check_Qty { get; set; }
        public int Applicant_UID { get; set; }
        public string ApplicantUser { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Applicant_Date { get; set; }
        public int Check_Status { get; set; }
        public int Storage_InOut_UID { get; set; }
        public int Approver_UID { get; set; }
        public string ApproverUser { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Approver_Date { get; set; }
        public int Status_UID { get; set; }
        public string Status { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Plant_UID { get; set; }
        public string BG_Organization { get; set; }
        public string Plant { get; set; }
        public string FunPlant_Organization { get; set; }
    }
}
