﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_DefectCodeDailyNumDTO : BaseModel
    {
        public int OEE_DefectCodeDailyNum_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int OEE_StationDefectCode_UID { get; set; }
        public System.DateTime ProductDate { get; set; }
        public string TimeInterval { get; set; }
        public int DefectNum { get; set; }
        public int ShiftTimeID { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }
    }
}