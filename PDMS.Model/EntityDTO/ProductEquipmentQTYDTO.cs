using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ProductEquipmentQTYDTO : EntityDTOBase
    {
        public int? Product_Equipment_QTY_UID { get; set; }
        public int? FlowChart_Detail_UID { get; set; }
        public int? Flowchart_Detail_ME_UID { get; set; }
        public int? Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int? Father_UID { get; set; }
        public int? Child_UID { get; set; }
        public int? System_FunPlant_UID { get; set; }
        public string Product_Phase { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ProductDate { get; set; }
        public int? Qty { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int Process_Seq { get; set; }
        public string FunPlant { get; set; }
        public string Process_Station { get; set; }
        public string Process { get; set; }
        public int Sub_ProcessSeq { get; set; }
        public string SubProcess { get; set; }
        public string Equipment_Name { get; set; }
        public int LanguageID { get; set; }

    }
}
