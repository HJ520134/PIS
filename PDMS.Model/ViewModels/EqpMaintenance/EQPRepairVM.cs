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
    public class EQPRepairVM : BaseModel
    {
        public List<string> eqplocation { get; set; }
        public List<SystemProjectDTO> optypes {get;set;}
        public bool showapply { get; set; }
        public bool showrepair { get; set; }
        public IEnumerable<EnumerationDTO> errorinfo { get; set; }
        public IEnumerable<EQPUserTableDTO> userinfo { get; set; }
        public IEnumerable<MaterialInfoDTO> materialinfo { get; set; }
        public List<string> unitmat { get; set; }
        public IEnumerable<EquipmentInfoDTO> equipmentinfo { get; set; }
        public string iseqp_user { get; set; }
        public string optype { get; set; }
        public bool needcost { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Apply_Time { get; set; }
    }

    public class EQPRepairSearchVM : BaseModel
    {
        public List<string> eqplocation { get; set; }
        public  List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public bool showapply { get; set; }
        public bool showrepair { get; set; }
        public IEnumerable<EnumerationDTO> errorinfo { get; set; }
        public IEnumerable<EQPUserTableDTO> userinfo { get; set; }
        public IEnumerable<MaterialInfoDTO> materialinfo { get; set; }
        public List<string> unitmat { get; set; }
        public IEnumerable<EquipmentInfoDTO> equipmentinfo { get; set; }
        public string iseqp_user { get; set; }
        public string optype { get; set; }
        public bool needcost { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Apply_Time { get; set; }
        public int bgUID { get; set; }
        public int funplantUID { get; set; }
        public int plantUID { get; set; }
        //價格顯示限制
        public bool showprice { get; set; }
    }
    public  class PlantVM
    {
       
       public    string Plant { get; set; }
    
       public  int Plant_OrganizationUID { get; set; }
    }
    public class BGVM
    {
        public string BG { get; set; }
        public int BG_OrganizationUID { get; set; }
    }
}
