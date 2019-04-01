using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemOrgDTO : EntityDTOBase
    {
        public int Organization_UID { get; set; }
        public string Organization_ID { get; set; }
        public string Organization_Name { get; set; }
        public string OrgManager_Name { get; set; }
        public string OrgManager_Tel { get; set; }
        public string OrgManager_Email { get; set; }
        public string Organization_Desc { get; set; }
        public string Cost_Center { get; set; }
        public  int ChildOrg_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }
    }
}
