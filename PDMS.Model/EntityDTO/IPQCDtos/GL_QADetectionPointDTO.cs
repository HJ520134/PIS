using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_QADetectionPointDTO : BaseModel
    {

        public int QADetectionPointID { get; set; }
        public int WIP { get; set; }
        public string ScanIN { get; set; }
        public string ScanOUT { get; set; }
        public string ScanNG { get; set; }
        public string ScanBACK { get; set; }
        public bool IsEnabled { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public int LineID { get; set; }
        public bool IsBirth { get; set; }
        public bool IsOutput { get; set; }
        public bool IsTest { get; set; }
        public int Seq { get; set; }
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
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }

        public DateTime Modified_Date { get; set; }

        public string Modifieder { get; set; }

        public int Modified_UID { get; set; }
    }
}
