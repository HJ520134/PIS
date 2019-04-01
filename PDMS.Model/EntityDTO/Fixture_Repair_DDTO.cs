using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class Fixture_Repair_DDTO : EntityDTOBase
    {
        public int? Fixture_Repair_D_UID { get; set; }
        public int Fixture_Repair_M_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string ShortCode { get; set; }
        public string Line_Name { get; set; }
        public string Workshop_Name { get; set; }
        public string Fixture_Name{ get; set; }
        public int Repair_Staff_UID { get; set; }
        public string Repair_Staff_NTID{ get; set; }
        public string Repair_Staff_Name { get; set; }
        public DateTime? Completion_Date { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int? Created_UID { get; set; }
        public DateTime Created_Date { get; set; }
        
        public ICollection<Fixture_Repair_D_DefectDTO> Fixture_Repair_D_DefectDTOList { get; set; }
        public EnumerationDTO StatusEnumeration { get; set; }
        public FixtureDTO Fixture_M { get; set; }

        //自定义
    }
}
