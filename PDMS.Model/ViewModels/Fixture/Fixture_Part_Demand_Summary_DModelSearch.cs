﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Part_Demand_Summary_DModelSearch : BaseModel
    {
        public int? Fixture_Part_Demand_Summary_D_UID { get; set; }
        public int? Fixture_Part_Demand_Summary_M_UID { get; set; }
        public int? Fixture_Part_UID { get; set; }
        public decimal? Storage_Qty { get; set; }
        public decimal? Total_Gross_Demand { get; set; }
        public decimal? Total_User_Adjustments_Qty { get; set; }
        public decimal? Total_Actual_Demand { get; set; }

        public decimal? Safe_Storage_Qty { get; set; }
        public bool? Is_Deleted { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}