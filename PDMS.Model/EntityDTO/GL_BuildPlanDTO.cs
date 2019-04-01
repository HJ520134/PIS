
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public  class GL_BuildPlanDTO : EntityDTOBase
    {
        public int BuildPlanID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int? AssemblyID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PlanOutput { get; set; }
        public int? BPSourceID { get; set; }
        public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int? PlanHC { get; set; }
        public int? ActualHC { get; set; }
        public string CustomerName { get; set; }
        public string LineName { get; set; }

        public int UpdateType { get; set; }
        public GL_LineDTO GL_Line { get; set; }
        // Plant And Site
        public SystemOrgDTO System_Organization { get; set; }
        // BG Code
        public SystemOrgDTO System_Organization1 { get; set; }
        // Function Plant
        public SystemOrgDTO System_Organization2 { get; set; }
        // System Users
        public SystemUserDTO System_Users { get; set; }
    }
}
