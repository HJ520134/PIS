using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public  class Vendor_InfoDTO : EntityDTOBase
    {
        public int Vendor_Info_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public string Vendor_ID { get; set; }
        public string Vendor_Name { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
    
    }
}
