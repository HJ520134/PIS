using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageReportDTO : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Material_Uid { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
        public int Warehouse_Type_UID { get; set; }
        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public decimal Balance_Qty { get; set; }
        public decimal In_Qty { get; set; }
        public decimal Out_Qty { get; set; }
        public decimal Last_Qty { get; set; }
        public DateTime intMonth { get; set; }
    }
}
