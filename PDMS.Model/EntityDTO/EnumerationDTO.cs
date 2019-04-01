using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class EnumerationDTO:EntityDTOBase
    {
        public int Enum_UID { get; set; }
        public string Enum_Type { get; set; }
        public string Enum_Name { get; set; }
        public string Enum_Value { get; set; }
        public string Decription { get; set; }
    }
}
