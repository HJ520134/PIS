using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels
{
    public class ProjectVM: BaseModel
    {
        public int? Organization_Name { get; set; }
        public int Organization_UID { get; set; }
        public int Project_UID { get; set; }
        public string Project { get; set; }
        public string MESProject { get; set; }
        public string Customer { get; set; }
        public string Product_Phase { get; set; }
        public string OP_TYPES { get; set; }
        public string Project_Type { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
        public string Modified_User { get; set; }
        public int OrgID { get; set; }
        public string Plant { get; set; }
    }

    public class ProjectVMDTO : EntityDTOBase
    {
        public string Organization_Name { get; set; }
        public int Project_UID { get; set; }
        public string Project { get; set; }
        public string MESProject { get; set; }
        public string Customer { get; set; }
        public string Product_Phase { get; set; }
        public string OP_TYPES { get; set; }
        public string Project_Type { get; set; }
        public int Organization_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
        public string Modified_User { get; set; }
        public int OrgID { get; set; }
    }
}
