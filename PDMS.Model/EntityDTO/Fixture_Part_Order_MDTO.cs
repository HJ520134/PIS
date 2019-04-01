using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_Order_MDTO : BaseModel
    {
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Order_ID { get; set; }
        public DateTime? Order_Date { get; set; }
        public bool Is_Complated { get; set; }
        public string Remarks { get; set; }
        public bool Del_Flag { get; set; }
        public int Created_UID { get; set; }
        public DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string ModifyName { get; set; }
        public bool Is_SubmitFlag { get; set; }

    }
}
