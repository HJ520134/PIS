using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProductionPlanning
{
    public class ProductionSchedulNPIVM : BaseModel
    {
        public int? Production_Schedul_NPI_UID { get; set; }

        public int Flowchart_Detail_ME_UID { get; set; }

        public int FlowChart_Master_UID { get; set; }

        public int FlowChart_Version { get; set; }

        public string Project_Name { get; set; }

        public string Product_Phase { get; set; }

        public int Process_Seq { get; set; }

        public string Process { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Product_Date { get; set; }

        public int? Input { get; set; }
    }
}
