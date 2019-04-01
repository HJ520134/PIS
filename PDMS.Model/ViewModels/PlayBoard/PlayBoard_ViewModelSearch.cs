using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class PlayBoard_ViewModelSearch : BaseModel
    {
        public int? PlayBoard_View_ID { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string SettingActionName { get; set; }
        public string Desc { get; set; }
        public string ParameterDesc { get; set; }
        public bool? IsJsonParameterNeed { get; set; }
        public bool? IsEnabled { get; set; }
        public string Title { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
    }
}
