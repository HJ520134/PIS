﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class MaintenancePlanDTO: EntityDTOBase
    {
        public int Maintenance_Plan_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Maintenance_Type { get; set; }
        public string Cycle_ID { get; set; }
        public int? Cycle_Interval { get; set; }
        public string Cycle_Unit { get; set; }
        public int? Lead_Time { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Start_Date { get; set; }
        public System.DateTime Start_Date_start { get; set; }
        public System.DateTime Start_Date_end { get; set; }
        public int? Tolerance_Time { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> Last_Execution_Date { get; set; }
        public System.DateTime Last_Execution_Date_start { get; set; }
        public System.DateTime Last_Execution_Date_end { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> Next_Execution_Date { get; set; }
        public string Next_Execution_Date_Nvar { get; set; }
        public System.DateTime Next_Execution_Date_start { get; set; }
        public System.DateTime Next_Execution_Date_end { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public bool needSearchEnable { get; set; }
    }
}
