using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class PlayBoard_PlayTimeDTO : EntityDTOBase
    {
        public int PlayBoard_PlayTime_ID { get; set; }
        public int PlayBoard_Setting_ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        //自定义栏位
    }
}
