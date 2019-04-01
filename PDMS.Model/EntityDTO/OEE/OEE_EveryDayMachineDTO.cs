using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{

    public class OEE_EveryDayMachineDTO : EntityDTOBase
    {
        public int OEE_EveryDayMachine_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int FixtureNum { get; set; }
        public double PORCT { get; set; }
        public double ActualCT { get; set; }
        public int OutPut { get; set; }
        public double TotalAvailableHour { get; set; }
        public double PlannedHour { get; set; }
        public int? ShiftTimeID { get; set; }
        public string  ShiftName { get; set; }
        public System.DateTime Product_Date { get; set; }
        public DateTime UpdateTime { get; set; }
        public string MachineName { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string ResetTime { get; set; }
        public Nullable<int> Is_DownType { get; set; }
        public Nullable<bool>  AbnormalDFCode { get; set; }
        public Nullable<bool> Is_Offline { get; set; }
        public string OEEDashBoardTarget { get; set; }
        public string OrganitionName { get; set; }
    }


    public class OEE_ReprortSearchModel : EntityDTOBase
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int EQP_Uid { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ShiftTimeID { get; set; }
        public int VersionNumber { get; set; }//0 内部版，1 外部版本
        public int OEE_DownTimeType_UID { get; set; }
        public string Param_LineName { get; set; }
        public string Param_StationName { get; set; }

        public int? languageID { get; set; }

        public int OrderByRule { get; set; }
        public int PagePieSize { get; set; }
        public int PageNum { get; set; }
    }


    public class LocalEveryDayMachine
    {
        public int MachineNo { get; set; }
        public int FixtureNum { get; set; }
        public double PORCT { get; set; }
        public double ActualCT { get; set; }
        public int OutPut { get; set; }
        public double TotalAvailableHour { get; set; }
        public double PlannedHour { get; set; }
    }

    public class LocalCNCInfo
    {
        public string MachineNo { get; set; }
        public int OutPut { get; set; }
        public double ActualCT { get; set; }
        public    string ResetTime { get; set; }
    }

    public class LoacalMachineDefect
    {
        public string MachineNo { get; set; }
        public string DefectCode { get; set; }
        public int DownTime { get; set; }
    }

    public class ProPerCT
    {
        public string MachineName { get; set; }
        public double PorCT { get; set; }
    }
}
