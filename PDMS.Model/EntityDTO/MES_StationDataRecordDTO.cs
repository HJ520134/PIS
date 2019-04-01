using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class MES_StationDataRecordDTO : BaseModel
    {
        public int MES_StationDataRecord_UID { get; set; }
        public string Date { get; set; }
        public string TimeInterVal { get; set; }
        public string StartTimeInterval { get; set; }
        public string EndTimeInterval { get; set; }
        public int PIS_ProcessID { get; set; }
        public string PIS_ProcessName { get; set; }
        public string MES_ProcessID { get; set; }
        public string MES_ProcessName { get; set; }
        public string ProjectName { get; set; }
        public string ProcessType { get; set; }
        public int ProductQuantity { get; set; }
        public string Color { get; set; }

    }

    public class MES_PISParamDTO : BaseModel
    {
        public int MES_StationDataRecord_UID { get; set; }
        public string Date { get; set; }
        public string TimeInterVal { get; set; }
        public string StartTimeInterval { get; set; }
        public string EndTimeInterval { get; set; }
        public int PIS_ProcessID { get; set; }
        public string PIS_ProcessName { get; set; }
        public string MES_ProcessID { get; set; }
        public int pis_WIPNum { get; set; }
        public string MES_ProcessName { get; set; }
        public string ProjectName { get; set; }
        public string ProcessType { get; set; }
        public string Color { get; set; }
        public int PIS_Pick_Number { get; set; }
        public int PIS_NG_Number { get; set; }
        public int PIS_Rework_Number { get; set; }
        public int PIS_GP_Number { get; set; }
        public int FlowChart_Version { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSyncNG { get; set; }

    }
}
