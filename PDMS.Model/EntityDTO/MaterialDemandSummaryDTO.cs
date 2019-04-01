using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class MaterialDemandSummaryDTO:BaseModel
    {
        public int Material_Demand_Summary_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Material_UID { get; set; }
        public decimal NormalDemand_Qty { get; set; }
        public decimal SparepartsDemand_Qty { get; set; }
        public decimal Repair_Demand_Qty { get; set; }
        public decimal Be_Purchase_Qty { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Calculation_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Demand_Date { get; set; }
        public int Applicant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approver_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Approver_Date { get; set; }

        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public string Status { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string ApplicantUser { get; set; }
        public string ApproverUser { get; set; }
        public int Plant_UID { get; set; }
    }
}
