using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    /// <summary>
    /// 品质良率报表Dto
    /// </summary>
    public class IPQCQualityReportDto: BaseModel
    {
  
        public int IPQCQualityReport_UID { get; set; }
        public int StationID { get; set; }
        public int ShiftID { get; set; }
        public string TimeInterval { get; set; }
        public DateTime ProductDate { get; set; }
        public double FirstYield { get; set; }
        public double FirstTargetYield { get; set; }
        public double SecondYield { get; set; }
        public double SecondTargetYield { get; set; }
        public int InputNumber { get; set; }
        public int TestNumber { get; set; }
        public int FirstPassNumber { get; set; }
        public int SecondPassNumber { get; set; }
        public int RepairNumber { get; set; }
        public int NGNumber { get; set; }
        public int WIP { get; set; }
        public int TimeIntervalIndex { get; set; }
        public System.DateTime ModifyTime { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
    }


    /// <summary>
    /// 不良明细Dtos
    /// </summary>
    public class IPQCQualityDetialDto : BaseModel
    {
        public int IPQCQualityDetial_UID { get; set; }
        public int StationID { get; set; }
        public int ShiftID { get; set; }
        public DateTime ProductDate { get; set; }
        public string TimeInterval { get; set; }
        public int TimeIntervalIndex { get; set; }
        public string NGName { get; set; }
        public int NGNumber { get; set; }
        public string NGType { get; set; }
        public System.DateTime ModifyTime { get; set; }
    }

}
