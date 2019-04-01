using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_StationDTO : EntityDTOBase  
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
        public int LineID { get; set; }
        public bool IsBirth { get; set; }
        public bool IsOutput { get; set; }
        public bool IsTest { get; set; }
        public int Seq { get; set; }
        public bool IsEnabled { get; set; }
        public int CustomerID { get; set; }
        public string ProjectName { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string LineName { get; set; }
        public string MESStationName { get; set; }
        public string MESLineName { get; set; }
        public string MESProjectName { get; set; }
        public bool LineIsEnabled { get; set; }
        public decimal CycleTime { get; set; }
        public decimal LineCycleTime { get; set; }
        public int Binding_Seq { get; set; }
        public bool IsGoldenLine { get; set; }
        public bool IsOEE { get; set; }
        public bool IsOne { get; set; }
        public bool IsTwo { get; set; }
        public bool IsThree { get; set; }
        public bool IsFour { get; set; }
        public bool IsFive { get; set; }
        public string DashboardTarget { get; set; }
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
    }
}
