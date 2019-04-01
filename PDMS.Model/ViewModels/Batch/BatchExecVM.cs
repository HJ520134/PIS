using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Batch
{
    public class BatchExecVM
    {
        public int System_Schedule_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int System_Module_UID { get; set; }
        public int Function_UID { get; set; }
        public string Cycle_Unit { get; set; }
        public string Exec_Moment { get; set; }
        public string Users_PIC_UIDs { get; set; }
        public string System_PIC_UIDs { get; set; }
        public string Role_UIDs { get; set; }
        public string Exec_Time { get; set; }
        public string Function_Name { get; set; }
        public Nullable<System.DateTime> Last_Execution_Date { get; set; }
        public DateTime Next_Execution_Date { get; set; }
        public Nullable<bool> Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }
}
