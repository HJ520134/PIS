using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_LineDTO : EntityDTOBase
    {

        public int LineID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string ProjectName { get; set; }
        public string MESProjectName { get; set; }
        public string Phase { get; set; }
        public string LineName { get; set; }
        public string MESLineName { get; set; }
        public int CustomerID { get; set; }
        public int Seq { get; set; }
        public bool IsEnabled { get; set; }
        public decimal CycleTime { get; set; }

        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }

      // Plant And Site
        public SystemOrgDTO System_Organization { get; set; }
        // BG Code
        public SystemOrgDTO System_Organization1 { get; set; }
        // Function Plant
        public SystemOrgDTO System_Organization2 { get; set; }
        // Customer
        public SystemProjectDTO System_Project { get; set; }
        // System Users
        public SystemUserDTO System_Users { get; set; }
        
        public List<GL_LineShiftResposibleUserDTO> GL_LineShiftResposibleUserList { get; set; }
    }

    public class WipGroupLineItem
    {
        public int LineID { get; set; }
        public string LineName { get; set; }
        public int CustomerID { get; set; }
        public int? LineParent_ID { get; set; }
        public int? LineGroup_UID { get; set; }
        public string GroupLineName { get; set; }
    }
}
