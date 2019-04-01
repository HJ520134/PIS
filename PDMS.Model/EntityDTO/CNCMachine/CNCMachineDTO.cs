using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class CNCMachineDTO : EntityDTOBase
    {
        public int CNCMachineUID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? EQP_Uid { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_ID { get; set; }
        public int Project_UID { get; set; }
        public bool Is_Enable { get; set; }

        //自定义
        public string Equipment { get; set; }
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string ProjectName { get; set; }
        public string Modifyer { get; set; }

        public  DateTime? QueryTime { get; set; }
    }
}
