using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;

namespace PDMS.Model
{
    public class SystemOrgBomDTO : EntityDTOBase
    {
        public int OrganizationBOM_UID { get; set; }
        public int? ParentOrg_UID { get; set; }
        public int Order_Index { get; set; }
        public int ChildOrg_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }



    }
}
