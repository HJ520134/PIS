using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Common
{
    public class LanguageVM
    {
        public SystemLanguageDTO CurrentLanguage { get; set; }
        public List<SystemLanguageDTO> Languages { get; set; }
    }
}
