using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    //查询模型, 值类型要是可空
    public class System_ProjectModelSearch : BaseModel
    {
        public int? Project_UID { get; set; }
        public string Project_Code { get; set; }
        public int? BU_D_UID { get; set; }
        public string Project_Name { get; set; }
        public string Product_Phase { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? Closed_Date { get; set; }
        public int? Modified_UID { get; set; }
        public System.DateTime? Modified_Date { get; set; }
        public string OP_TYPES { get; set; }
        public int? Organization_UID { get; set; }
        public string Project_Type { get; set; }
        public string MESProject_Name { get; set; }
    }
}
