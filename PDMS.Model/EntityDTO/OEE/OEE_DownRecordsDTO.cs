using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_DownRecordsDTO : BaseModel
    {
        public string StartTime { get; set; }
        public string EndTIme { get; set; }
        public string MachineName { get; set; }
        public string ErrorCode { get; set; }

        public int count { get; set; }
    }

}
