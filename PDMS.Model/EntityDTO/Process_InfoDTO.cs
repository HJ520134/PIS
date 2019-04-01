using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class Process_InfoDTO : EntityDTOBase
    {
        public int Process_Info_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Process_ID { get; set; }
        public string Process_Name { get; set; }
        public string Process_Desc { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string BGName { get; set; }
        public string FunPlantName { get; set; }
    }
}
