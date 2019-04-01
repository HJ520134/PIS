using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Part_UseTimesModelSearch : BaseModel
    {
        public int? Fixture_Part_UseTimes_UID { get; set; }
        public int? Fixture_M_UID { get; set; }
        public int? Fixture_Part_Setting_D_UID { get; set; }
        public int? Fixture_Part_UseTimesCount { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
