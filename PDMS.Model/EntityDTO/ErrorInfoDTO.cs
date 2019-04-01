using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class ErrorInfoDTO: EntityDTOBase
    {
        public int Enum_UID { get; set; }
        public string ErrorType { get; set; }
        public string Value { get; set; }
    }
}
