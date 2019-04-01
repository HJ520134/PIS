using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class PlantModelSearch : BaseModel
    {
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        // 廠別代碼Plant Code
        public string Name_0 { get; set; }
        //公司中文名稱Plant Name(ZH)
        public string Name_1 { get; set; }
        public string PlantManager_Name { get; set; } 
        public DateTime Reference_Date { get; set; }
        public int? query_types { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }
}
