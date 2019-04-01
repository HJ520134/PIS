using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public   class ActualAndPlanDTO : EntityDTOBase  
    {

        public int StationID { get; set; }
        public string StationName { get; set; }
        public string MESStationName { get; set; }
        public bool IsEnabled { get; set; }
        public int LineID { get; set; }
        public string LineName { get; set; }
        public string MESLineName { get; set; }
        public bool LineIsEnabled { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string MESCustomerName { get; set; }
        public bool CustomerIsEnabled { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int ShiftTimeID { get; set; }
        public string OutputDate { get; set; }
        public int PlanOutPut { get; set; }
        public decimal PlanLB { get; set; }
        public decimal PlanSMH { get; set; }
        public int PlanHC { get; set; }
        public decimal PlanUPPH { get; set; }
        public decimal PlanVAOLE { get; set; }
        public int ActualOutPut { get; set; }
        public decimal ActualLB { get; set; }
        public decimal ActualSMH { get; set; }
        public int ActualHC { get; set; }
        public decimal ActualUPPH { get; set; }
        public decimal ActualVAOLE { get; set; }
        public decimal SumTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public  DateTime EndDateTime { get; set; }
        public string DateTimeNOW { get; set; }
    }
}
