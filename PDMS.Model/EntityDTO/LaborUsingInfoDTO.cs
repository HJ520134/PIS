using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class LaborUsingInfoDTO: EntityDTOBase
    {
        public int Labor_Using_Uid { get; set; }
        public int Repair_Uid { get; set; }
        public int EQPUser_Uid { get; set; }
        public Nullable<int> Modified_EQPUser_Uid { get; set; }
    }
}
