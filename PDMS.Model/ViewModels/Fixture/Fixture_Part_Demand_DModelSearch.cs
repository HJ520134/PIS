﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Part_Demand_DModelSearch : BaseModel
    {
        public int? Fixture_Part_Demand_D_UID { get; set; }
        public int? Fixture_Part_Demand_M_UID { get; set; }
        public string Fixture_NO { get; set; }
        public int? Fixture_Part_UID { get; set; }
        public int? Line_Qty { get; set; }
        public decimal? Line_Fixture_Ratio_Qty { get; set; }
        public decimal? Fixture_Part_Qty { get; set; }
        public decimal? Fixture_Part_Life { get; set; }
        public decimal? Gross_Demand { get; set; }
        public decimal? User_Adjustments_Qty { get; set; }
        public int? Purchase_Cycle { get; set; }
        public decimal? Safe_Storage_Qty { get; set; }
        public bool? Is_Deleted { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
