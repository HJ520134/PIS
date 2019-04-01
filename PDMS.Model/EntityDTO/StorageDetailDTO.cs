using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class StorageDetailDTO: EntityDTOBase
    {
        public int Storage_InOut_Detaill_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int InOut_Type_UID { get; set; }
        public int Storage_Detail_UID { get; set; }
        public string BG_Name { get; set; }
        public string Funplant { get; set; }
        public string Storage_Bound_ID { get; set; }
        public string Inout_Type { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime InOut_Date { get; set; }
        public decimal InOut_QTY { get; set; }
        public decimal Balance_Qty { get; set; }
        public string Bound_Type { get; set; }
        public string ModifiedUser { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public int Warehouse_Storage_UID { get; set; }
    }
    public class StorageSearchMod : EntityDTOBase
    {
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int InOut_Type_UID { get; set; }
        public int Storage_Detail_UID { get; set; }
        public string BG_Name { get; set; }
        public string Funplant { get; set; }
        public string Storage_Bound_ID { get; set; }
        public string Inout_Type { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime InOut_Date { get; set; }
        public decimal InOut_QTY { get; set; }
        public decimal Balance_Qty { get; set; }
        public string Bound_Type { get; set; }
        public string ModifiedUser { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public int Plant_UID { get; set; }


    }
}
