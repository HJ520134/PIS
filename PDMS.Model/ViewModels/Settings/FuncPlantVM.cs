using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FuncPlantMaintanance : EntityDTOBase
    { 
        public int System_FuncPlant_UID { get; set; }
        public string FunPlant { get; set; }
        public string Plant { get; set; }
        public string OPType { get; set; }
        public string Plant_Manager { get; set; }
        public string FuncPlant_Context { get; set; }
        public string Organization_ID { get; set; }

    }

    public class FuncPlantSearchModel : BaseModel
    {
        public string FunPlant { get; set; }
        public string Plant { get; set; }
        public string OPType { get; set; }
        public string FuncPlant_Manager { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }

}
