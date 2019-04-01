using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class OrgModelSearch : BaseModel
    {
        public string Organization_ID { get; set; }
        public string Organization_Name { get; set; }
        public string Organization_Desc { get; set; }
        public string OrgManager_Name { get; set; }
        public string OrgManager_Tel { get; set; }
        public string OrgManager_Email { get; set; }
        public string Cost_Center { get; set; }
        public DateTime Reference_Date { get; set; }
        public int? query_types { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }
}
