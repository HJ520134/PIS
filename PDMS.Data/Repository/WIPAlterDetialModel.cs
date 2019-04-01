using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data
{
   public class WIPAlterDetialModel
    {
        public string Part_Types { get; set; }
        public int? Organization_UID { get; set; }
        public string User_Name { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string Project_Name { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }

        public string Place { get; set; }
        public int? Change_UID { get; set; }
        public string Change_Type { get; set; }
        public int? Product_UID { get; set; }
        public int? WIP_Old { get; set; }
        public int? WIP_Add { get; set; }
        public string Comment { get; set; }
        public int? Modified_UID { get; set; }

        public DateTime Modified_Date { get; set; }

        public string alterTime { get; set; }

        public int? FlowChart_Detail_UID { get; set; }

        public string Organization_Name { get; set; }
    }
}
