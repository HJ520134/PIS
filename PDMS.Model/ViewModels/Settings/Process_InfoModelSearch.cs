using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Process_InfoModelSearch : BaseModel
    {
        public int? Process_Info_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Process_ID { get; set; }
        public string Process_Name { get; set; }
        public string Process_Desc { get; set; }
        public bool? Is_Enable { get; set; }
    }
}
