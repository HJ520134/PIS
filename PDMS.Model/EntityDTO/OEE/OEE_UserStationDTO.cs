using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_UserStationDTO:BaseModel
    {
        public int OEE_UserStation_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int KeyInNG_User_UID { get; set; }
        public int Project_UID { get; set; }
        public string Project_Name { get; set; }
        public int Line_ID { get; set; }
        public string Line_Name { get; set; }
        public int StationID { get; set; }
        public string Station_Name { get; set; }
        public string User_NTID { get; set; }
        public int Modified_UID { get; set; }
        public string Modified_name { get; set; }
        public DateTime Modified_Date { get; set; }
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }
    }
}
