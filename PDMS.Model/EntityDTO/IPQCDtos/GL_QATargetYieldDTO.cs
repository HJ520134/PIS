using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_QATargetYieldDTO : BaseModel
    {

        public int GLQATargetYieldID { get; set; }
        public int StationID { get; set; }
        public string TargetYieldDate { get; set; }
        public decimal TargetYield { get; set; }
        public int Tag { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string StationName { get; set; }
        public string LineName { get; set; }
        public string ProjectName { get; set; }
        public string Modifieder { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
    }
}
