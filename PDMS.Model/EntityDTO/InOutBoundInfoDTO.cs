using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class InOutBoundInfoDTO : EntityDTOBase
    {
        public int Storage_Bound_UID { get; set; }
        public int Storage_Inbound_Type_UID { get; set; }
        public int Storage_Outbound_Type_UID { get; set; }
        public string InOut_Type { get; set; }
        public string In_Out_Type { get; set; }
        public string Storage_Bound_ID { get; set; }
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public decimal Bound_Qty { get; set; }
        public decimal OK_Qty { get; set; }
        public int Applicant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Applicant_Date { get; set; }
        public int Approver_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Approver_Date { get; set; }
        public string Status { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Warehouse_ID { get; set; }
        public string Name_ZH { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string Accepter { get; set; }

        public string Storage_Bound_Type { get; set; }
        public int Storage_Bound_Type_UID { get; set; }
        public string ModifiedUser { get; set; }
        public string ApproverUser { get; set; }
        public int Status_UID { get; set; }
        public int Outbound_Account_UID { get; set; }
        public string PU_NO { get; set; }
        public string Issue_NO { get; set; }
        public string Repair_id { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Plant_UID { get; set; }
        public  List<OutBoundInfoDTO> OutBoundInfos { get; set; }
    }

    public class OutBoundInfoDTO : EntityDTOBase
    {
        public int Storage_Outbound_M_UID { get; set; }
        public string Storage_Bound_ID { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string PartType { get; set; }
        public decimal Be_Out_Qty { get; set; }
        public decimal Outbound_Qty { get; set; }
        public decimal Inventory_Qty { get; set; }
    }

}
