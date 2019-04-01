using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class EQPTypeDTO : EntityDTOBase
    {
        public int EQP_Type_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public string EQP_Type1 { get; set; }
        public string Type_Desc { get; set; }
        public bool Is_Enable { get; set; }
        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
        public int Plant_UID { get; set; }
    }


    public class EQPTypeBaseDTO : EntityDTOBase
    {
      
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string EQP_Type1 { get; set; }
      
    }
}
