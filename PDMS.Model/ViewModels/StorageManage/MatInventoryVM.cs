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
    public class MatInventoryVM
    {
        public List<SystemOrgDTO> oporg { get; set; }
        public List<EnumerationDTO> Types { get; set; }
        public List<WarehouseDTO> warst { get; set; }
    }
}
