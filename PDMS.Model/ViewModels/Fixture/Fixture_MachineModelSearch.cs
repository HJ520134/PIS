using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_MachineModelSearch : BaseModel
    {
        public int? Fixture_Machine_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Machine_ID { get; set; }
        public string Equipment_No { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_Desc { get; set; }
        public string Machine_Type { get; set; }
        public int? Production_Line_UID { get; set; }
        public bool? Is_Enable { get; set; }

        //TODO 2018/09/18 steven 增加EQP_UID
        public int? EQP_Uid { get; set; }
    }
}
