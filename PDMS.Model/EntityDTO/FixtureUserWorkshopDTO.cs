﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class FixtureUserWorkshopDTO: EntityDTOBase
    {
        public int Fixture_User_Workshop_UID { get; set; }
        public int Account_UID { get; set; }
        public int Workshop_UID { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public string Workshop_ID { get; set; }
        public string Workshop_Name { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }

        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public bool Is_Enable { get; set; }
        public bool needSearchEnable { get; set; }
    }
}
