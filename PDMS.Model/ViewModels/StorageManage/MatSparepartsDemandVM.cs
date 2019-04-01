using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class MatSparepartsDemandVM
    {
        public List<SystemOrgDTO> Orgs { get; set; }
        public List<MaterialInfoDTO> mats { get; set; }
        public List<EnumerationDTO> enums { get; set; }
    }
}
