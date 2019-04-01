using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class SystemBUMDTO : EntityDTOBase
    {
        public int BU_M_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public string BUManager_Name { get; set; }

        public string BUManager_Tel { get; set; }

        public string BUManager_Email { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? End_Date { get; set; }

    }
}
