using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class System_LocalizedPropertyVM : BaseModel
    {
        public IList<SystemLanguageDTO> LanguageList { get; set; }
    }
}
