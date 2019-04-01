using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class ProjectSearchModel:BaseModel
    {
        public int Organization_Name { get; set; }
        public int Organization_UID { get; set; }
        public string Project { get; set; }
        public string MESProject { get; set; }
        public string Customer { get; set; }
        public string Product_Phase { get; set; }
        public string OP_TYPES { get; set; }
        public int OrgID { get; set; }
        public int CurrentUser { get; set; }
    }
}
