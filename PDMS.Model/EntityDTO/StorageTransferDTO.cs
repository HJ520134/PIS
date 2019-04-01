using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageTransferDTO:BaseModel
    {
        public int Storage_Transfer_UID { get; set; }
        public string Storage_Transfer_ID { get; set; }
        public int Material_Uid { get; set; }
        public int Out_Warehouse_Storage_UID { get; set; }
        public int In_Warehouse_Storage_UID { get; set; }
        public Nullable<decimal> Transfer_Price { get; set; }
        public int PartType_UID { get; set; }
        public string PartType { get; set; }
        public decimal Transfer_Qty { get; set; }
        public int Applicant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approver_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Approver_Date { get; set; }

        public string Status { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Out_Warehouse_ID { get; set; }
        public string Out_Rack_ID { get; set; }
        public string Out_Storage_ID { get; set; }
        public string In_Warehouse_ID { get; set; }
        public string In_Rack_ID { get; set; }
        public string In_Storage_ID { get; set; }
        public string Transfer_Type { get; set; }
        public string ApplicantUser { get; set; }
        public string ApproverUser { get; set; }

        public int BG_Organization_UID { get; set; }
        public int Plant_UID { get; set; }


        public int In_BG_Organization_UID{ get; set; }
        public  string In_BG_Organization { get; set; }
        public  int In_FunPlant_Organization_UID { get; set; }
        public  string In_FunPlant_Organization { get; set; }
        public int Out_BG_Organization_UID { get; set; }
        public string Out_BG_Organization { get; set; }
        public int Out_FunPlant_Organization_UID { get; set; }
        public string Out_FunPlant_Organization { get; set; }



    }
}
