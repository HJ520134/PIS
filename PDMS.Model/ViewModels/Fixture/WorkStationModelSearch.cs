using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class WorkStationModelSearch : BaseModel
    {
        public int? WorkStation_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string WorkStation_ID { get; set; }
        public string WorkStation_Name { get; set; }
        public string WorkStation_Desc { get; set; }
        public int Project_UID { get; set; }
        public int Process_Info_UID { get; set; }
        public bool? Is_Enable { get; set; }
    }
}