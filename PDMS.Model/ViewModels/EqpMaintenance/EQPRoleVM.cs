using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class EQPRoleVM : BaseModel
    {
        public string rolename { get; set; }
        public string optype { get; set; }
        public List<SystemProjectDTO> optypes {get;set;}
        public List<PlantVM> Plants { get; set; }
    }

}
