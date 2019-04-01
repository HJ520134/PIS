using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemOrgAndBomDTO : EntityDTOBase
    {
        public int OrganizationBOM_UID { get; set; }

        public int? ParentOrg_UID { get; set; }
        public string ParentOrg_ID { get; set; }
        public string Parent_Organization_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ParentOrg_BeginDate { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ParentOrg_EndDate { get; set; }

        public int ChildOrg_UID { get; set; }
        public string ChildOrg_ID { get; set; }
        public string Child_Organization_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime ChildOrg_BeginDate { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ChildOrg_EndDate { get; set; }

        public int Order_Index { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }


    }
}
