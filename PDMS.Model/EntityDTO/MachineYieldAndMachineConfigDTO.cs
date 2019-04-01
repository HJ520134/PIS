using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public  class MachineYieldAndMachineConfigDTO : EntityDTOBase
    {

      public List<Machine_YieldDTO> Machine_YieldDTOs { get; set; }

      public List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs { get; set; }

    }
}
