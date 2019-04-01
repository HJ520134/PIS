using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_IPQCQualityDetialDTO : BaseModel
    {
        public int IPQCQualityDetial_UID { get; set; }
        public int StationID { get; set; }
        public int ShiftID { get; set; }
        public DateTime ProductDate { get; set; }
        public string TimeInterval { get; set; }
        public int TimeIntervalIndex { get; set; }
        public string NGName { get; set; }
        public int NGNumber { get; set; }
        public string NGType { get; set; }
        public System.DateTime ModifyTime { get; set; }
    }
}
