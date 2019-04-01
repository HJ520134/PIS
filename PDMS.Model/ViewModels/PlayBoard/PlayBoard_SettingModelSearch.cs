using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class PlayBoard_SettingModelSearch : BaseModel
    {
        public int? PlayBoard_Setting_ID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? PlayBoard_View_ID { get; set; }
        public int? PlaySeq { get; set; }
        public string JsonParameter { get; set; }
        public string Remark { get; set; }
        public bool? IsTiming { get; set; }
        public string TimingStart { get; set; }
        public string TimingEnd { get; set; }
        public int? Play_UID { get; set; }
        public bool? IsEnabled { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
