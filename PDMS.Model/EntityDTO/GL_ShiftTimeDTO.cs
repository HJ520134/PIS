using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_ShiftTimeDTO : EntityDTOBase
    {
        public int ShiftTimeID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string End_Time { get; set; }
        public bool IsEnabled { get; set; }
        public double Break_Time { get; set; }

        public int Sequence { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }

        // Plant And Site
        public SystemOrgDTO System_Organization { get; set; }
        // BG Code
        public SystemOrgDTO System_Organization1 { get; set; }
        // Function Plant
        public SystemOrgDTO System_Organization2 { get; set; }
        // System Users
        public SystemUserDTO System_Users { get; set; }
    }

    public class ShiftModel : BaseModel
    {
        public int ShiftTimeID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string End_Time { get; set; }
        public bool IsEnabled { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string  TimeInterval{ get; set; }
    }

   

}
