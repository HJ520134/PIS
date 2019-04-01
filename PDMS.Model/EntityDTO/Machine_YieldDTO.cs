using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Machine_YieldDTO : EntityDTOBase
    {
        public int Machine_Yield_UID { get; set; }
        public int Machine_Station_UID { get; set; }
        public string Machine_ID { get; set; }
        public Nullable<int> InPut_Qty { get; set; }
        public Nullable<int> NG_Qty { get; set; }
        public Nullable<int> NG_Point_Qty { get; set; }
        public Nullable<System.DateTime> StarTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> Yield_Qty { get; set; }
        public Nullable<decimal> Yield { get; set; }
        public Nullable<decimal> NO_Yield { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string MES_Customer_Name { get; set; }
        public string PIS_Customer_Name { get; set; }
        public string MES_Station_Name { get; set; }
        public string PIS_Station_Name { get; set; }
        public int Machine_Customer_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }
    }
}
