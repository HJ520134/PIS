using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Storage_Outbound_MDTO : BaseModel
    {
        public int Fixture_Storage_Outbound_M_UID { get; set; }
        public string Fixture_Storage_Outbound_ID { get; set; }
        public int Fixture_Storage_Outbound_Type { get; set; }
        public Nullable<int> Fixture_Repair_M_UID { get; set; }
        public Nullable<int> Outbound_Account_UID { get; set; }
        public string Remarks { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approve_UID { get; set; }
        public System.DateTime Approve_Date { get; set; }
    }
}
