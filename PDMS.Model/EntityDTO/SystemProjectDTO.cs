using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemProjectDTO : EntityDTOBase
    {
        public int Project_UID { get; set; }
        public string Project_Code { get; set; }
        public string OP_TYPES { get; set; }
        public int BU_D_UID { get; set; }
        public string Project_Name { get; set; }
        public string MESProject_Name { get; set; }
        public string Product_Phase { get; set; }
        public int Organization_UID { get; set; }
        public string Organization_Name { get; set; }
        public string Project_Type { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> Closed_Date { get; set; }
    }
    public class SystemProjectPlantDTO : EntityDTOBase
    {
        public int Project_UID { get; set; }
        public string Project_Code { get; set; }
        public int Plant_UID { get; set; }
        public string Plant { get; set; }
        public string OP_TYPES { get; set; }
        public int BU_D_UID { get; set; }
        public string Project_Name { get; set; }
        public string MESProject_Name { get; set; }
        public string Product_Phase { get; set; }
        public int Organization_UID { get; set; }
        public string Organization_Name { get; set; }
        public string Project_Type { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> Closed_Date { get; set; }
    }
}
