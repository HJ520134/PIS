using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public  class MES_PIS_SyncFailedRecordDTO: BaseModel
    {
        public int MES_PIS_SyncFailedRecord_UID { get; set; }
        public string SyncType { get; set; }
        public string SyncName { get; set; }
        public System.DateTime SyncTime { get; set; }
        public string SyncRequest { get; set; }
        public string SyncResult { get; set; }
        public bool Is_ManuallySuccess { get; set; }
        public int FailedNumber { get; set; }
        public int OperateID { get; set; }
        public System.DateTime OperateTime { get; set; }
    }
}
