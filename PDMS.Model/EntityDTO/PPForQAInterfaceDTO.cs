using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;

namespace PDMS.Model
{
    public class PPForQAInterfaceDTO
    {
        public int FlowChart_Detail_UID { get; set; }
        public int Input_Qty { get; set; }
        public int NG_Qty { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Color { get; set; }
        public string MaterielType { get; set; }
        public bool QAUsedFlag { get; set; }
        public System.DateTime Create_Date { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }
}
