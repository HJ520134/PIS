using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class SystemUserBusinessGroupDTO : EntityDTOBase
    {
        public int System_User_BU_UID { get; set; }

        public int Account_UID { get; set; }

        public int BU_M_UID { get; set; }

        public int BU_D_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? End_Date { get; set; }
    }
}
