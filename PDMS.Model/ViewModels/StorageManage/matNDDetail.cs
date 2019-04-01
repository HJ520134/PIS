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
    public class matNDDetail:BaseModel
    {
        public int Status_UID { get; set; }
        public int Material_Normal_Demand_UID { get; set; }
        public bool IsView { get; set; }
    }
}
