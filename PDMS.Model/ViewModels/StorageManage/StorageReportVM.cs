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
    public class StorageReportVM:BaseModel
    {
        public List<SystemOrgDTO> plants { get; set; }

        public List<EnumerationDTO> Types { get; set; }
    }
}
