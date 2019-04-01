using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Machine_Schedule_ConfigDTO : EntityDTOBase
    {
        
        public int Machine_Schedule_Config_UID { get; set; }
        public int Machine_Station_UID { get; set; }
        public string MES_Customer_Name { get; set; }
        public string MES_Station_Name { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<System.DateTime> StarTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public bool? Customer_Is_Enable { get; set; }
        public bool? Station_Is_Enable { get; set; }

        public string DataSourceType { get; set; }
    }
}
