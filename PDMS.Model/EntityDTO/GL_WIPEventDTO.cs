using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_WIPEventDTO : EntityDTOBase
    {
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int outPutCount { get; set; }
        public Nullable<int> AssemblyID { get; set; }
    }
}
