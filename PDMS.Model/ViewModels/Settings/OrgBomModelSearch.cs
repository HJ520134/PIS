using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class OrgBomModelSearch : BaseModel
    {
        public string ParentOrg_ID { get; set; }
        public string ParentOrg_Name { get; set; }
        public string ChildOrg_ID { get; set; }
        public string ChildOrg_Name { get; set; }
        public DateTime? Reference_Date { get; set; }
        public int? query_types { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }
}
