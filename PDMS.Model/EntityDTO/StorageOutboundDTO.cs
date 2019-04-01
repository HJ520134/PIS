using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageOutboundDTO: EntityDTOBase
    {
        public int Storage_Outbound_UID { get; set; }
        public int Storage_Outbound_Type_UID { get; set; }
        public string Storage_Outbound_Type { get; set; }
        public string Storage_Outbound_ID { get; set; }
        public int Material_Uid { get; set; }
        public Nullable<int> Repair_Uid { get; set; }
        public decimal Outbound_Qty { get; set; }
        public int Outbound_Account_UID { get; set; }
        public string Outbound_Account { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public Nullable<decimal> Outbound_Price { get; set; }
        public int Status_UID { get; set; }
        public string Desc { get; set; }
        public int Approver_UID { get; set; }
        public System.DateTime Approver_Date { get; set; }

        public Nullable<System.DateTime> Apply_Time { get; set; }
        public string Repair_id { get; set; }
        public string desc2 { get; set; }
        public string EQP_Location { get; set; }
        public string Equipment { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string Repair_Reason { get; set; }
        public string Status { get; set; }
    }
}
