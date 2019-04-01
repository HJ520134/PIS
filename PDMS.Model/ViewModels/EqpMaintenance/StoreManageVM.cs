using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class StoreManageVM: BaseModel
    {
        public IEnumerable<MaterialManageDTO> mats { get; set; }
        public List<string> locations { get; set; }
    }
}
