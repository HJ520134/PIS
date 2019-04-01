using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Part_Setting_MModelSearch : BaseModel
    {
        public int? Fixture_Part_Setting_M_UID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Fixture_NO { get; set; }
        public int? Line_Qty { get; set; }
        public decimal? Line_Fixture_Ratio_Qty { get; set; }
        public int? UseTimesScanInterval { get; set; }
        public bool? Is_Enable { get; set; }
    }
}
