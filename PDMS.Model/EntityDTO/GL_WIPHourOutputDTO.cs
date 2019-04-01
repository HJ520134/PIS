using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_WIPHourOutputDTO : EntityDTOBase
    {
        public int WHOID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public string stationName { get; set; }

        public Nullable<int> AssemblyID { get; set; }
        public System.DateTime OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int HourIndex { get; set; }
        public int StandOutput { get; set; }
        public int ActualOutput { get; set; }
        public string ShiftDate { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string myRetriveDate { get; set; }
        public string MESStationName { get; set; }

        public string MESCustomerName { get; set; }
        public string ProjectName { get; set; }
        public string MESLineName { get; set; }
        public string LineName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LineType { get; set; }
        public int? LineParent_ID { get; set; }
    }

    public class TimeIntervalModel : EntityDTOBase
    {
        public string Station { get; set; }
        public int TatalOutPut { get; set; }
        public int TimeInterval1 { get; set; }
        public int TimeInterval2 { get; set; }
        public int TimeInterval3 { get; set; }
        public int TimeInterval4 { get; set; }
        public int TimeInterval5 { get; set; }
        public int TimeInterval6 { get; set; }
        public int TimeInterval7 { get; set; }
        public int TimeInterval8 { get; set; }
        public int TimeInterval9 { get; set; }
        public int TimeInterval10 { get; set; }
        public int TimeInterval11 { get; set; }
        public int TimeInterval12 { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public decimal HourStandardOutPut { get; set; }

    }


    public class WipEventModel : EntityDTOBase
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int LineId { get; set; }
        public string LineName { get; set; }
        public int StationId { get; set; }

        public string StationName { get; set; }

        public string SerialNumber { get; set; }
        public string TimeInterval { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class MESTimeInfo
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int actHourIndex { get; set; }
        public DateTime currentDayTime { get; set; }
        public string shiftdate { get; set; }
        public int ShiftID { get; set; }
    }


    public class GL_HoureOutputModel
    {
        public string StationName { get; set; }
        public string LineName { get; set; }
        public int Count { get; set; }
    }
}
