using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class matSDVM: BaseModel
    {
        public List<editSDUserAdjustQty> editList { get; set; }
        public int Modified_UID { get; set; }
        public int Status_UID { get; set; }
    }

    public class editSDUserAdjustQty
    {
        public int Material_Spareparts_Demand_UID { get; set; }
        public decimal User_Adjustments_Qty { get; set; }
    }
}
