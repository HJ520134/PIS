﻿using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class matRDVM:BaseModel
    {
        public List<editRDUserAdjustQty> editList { get; set; }
        public int Modified_UID { get; set; }
        public int Status_UID { get; set; }
    }
    public class editRDUserAdjustQty
    {
        public int Material_Repair_Demand_UID { get; set; }
        public decimal User_Adjustments_Qty { get; set; }
    }
}