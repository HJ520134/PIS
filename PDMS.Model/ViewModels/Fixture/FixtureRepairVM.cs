using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels
{

    public class FixtureRepairVM : BaseModel
    {
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public List<FixtureStatuDTO> FixtureStatus { get; set; }
        public  int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
}
