using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class Fixture_Repair_MDTO : EntityDTOBase
    {
        public int? Fixture_Repair_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Repair_NO { get; set; }
        public int Repair_Location_UID { get; set; }
        public string SentOut_Number { get; set; }
        public string SentOut_Name { get; set; }
        public int Receiver_UID { get; set; }
        public string Receiver_NTID { get; set; }
        public string Receiver_Name { get; set; }
        public DateTime SentOut_Date { get; set; }
        public int? Created_UID { get; set; }
        public DateTime Created_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Repair_LocationName { get; set; }
        public string Repair_Location_Name{ get; set; }


        public ICollection<Fixture_Repair_DDTO> Fixture_Repair_DDTOList { get; set; }
    }
}
