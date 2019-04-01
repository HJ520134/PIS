using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QualityAssurance_MgDataDTO : EntityDTOBase
    {
        public int QualityAssurance_MgData_UID { get; set; }
        public decimal FirstRejectionRate { get; set; }
        public decimal SecondRejectionRate { get; set; }
        public System.DateTime ProductDate { get; set; }
      
        public System.DateTime Create_Date { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public Nullable<int> FlowChart_Master_UID { get; set; }
        public Nullable<int> FlowChart_Detail_UID { get; set; }
        public Nullable<int> Process_seq { get; set; }
        public string Process { get; set; }
    }
}
