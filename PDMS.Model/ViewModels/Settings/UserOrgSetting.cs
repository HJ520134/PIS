using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class UserOrgModelSearch : BaseModel
    {
        public string User_NTID { get; set; }
        public string User_Name { get; set; }

        public string Organization_ID { get; set; }
        public string Organization_Name { get; set; }

        public DateTime? Reference_Date { get; set; }
        public int? query_types { get; set; }

        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }


    public class UserOrgItem : EntityDTOBase
    {
        public int System_UserOrgUID { get; set; }
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public string Organization_ID { get; set; }
        public  string Organization_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }
    }

    public class UserOrgEditModel : EntityDTOBase
    {
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }

        public IEnumerable<UserOrgWithOrg> UserOrgWithOrgs { get; set; }
    }

    public class UserOrgWithOrg : EntityDTOBase
    {
        public int Organization_UID { get; set; }
        public string Organization_ID { get; set; }
        public string Organization_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Org_Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> Org_End_Date { get; set; }

        public int System_UserOrgUID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime UserOrg_Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> UserOrg_End_Date { get; set; }

    }
}
