using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class UserInfoVM: BaseModel
    {
        public SystemUserDTO User { get; set; }
        public IEnumerable<PageUnauthorizedElement> PageUnauthorizedElements { get; set; }
        public List<SystemPlantDTO> PlantList { get; set; }
    }
}
