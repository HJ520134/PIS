using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ProductRequestStaffDTO : EntityDTOBase
    {
        public int? FlowChart_Detail_UID { get; set; }
        public int? Product_RequestStaff_UID { get; set; }
        public int Process_Seq { get; set; }
        public string FunPlant { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ProductDate { get; set; }
        public int? OP_Qty { get; set; }
        public int? Monitor_Staff_Qty { get; set; }
        public int? Technical_Staff_Qty { get; set; }
        public int? Material_Keeper_Qty { get; set; }
        public int? Others_Qty { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
    }
}
