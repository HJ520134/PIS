using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class Fixture_Repair_D_DefectDTO : EntityDTOBase
    {
        public int? Fixture_Repair_D_Defect_UID { get; set; }
        public int? Fixture_Repair_D_UID { get; set; }
        public int Defect_Code_UID { get; set; }
        public int Solution_UID { get; set; }
        public int Created_UID { get; set; }
        public DateTime Created_Date { get; set; }

        public string Defect_Code_Name { get; set; }
        public string Solution_Name { get; set; }
    }
}
