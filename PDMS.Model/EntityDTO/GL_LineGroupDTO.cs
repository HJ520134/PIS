using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_LineGroupDTO: EntityDTOBase
    {
        public int LineGroup_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? LineID { get; set; }
        public int? LineParent_ID { get; set; }
        public string LineParent_Name { get; set; }
        public string LineName { get; set; }
        public DateTime? _Modified_Date { get; set; }
        public int? _Modified_UID { get; set; }
        public int CustomerID { get; set; }
        public string Project_Name { get; set; }
        public string MESProject_Name { get; set; }
    }

}
