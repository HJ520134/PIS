using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class RoleModelSearch : BaseModel
    {
        public int Role_UID { get; set; }

        public string Role_ID { get; set; }

        public string Role_Name { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
    }
}
