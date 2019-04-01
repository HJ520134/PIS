using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProcessIDTraConfig
{
   public class ProcessIDTraConfigVM : BaseModel
    {
        public int ProcessTransformConfig_UID { get; set; }
        public int PIS_ProcessID { get; set; }
        public int Binding_Seq { get; set; }
        public string PIS_ProcessName { get; set; }
        public string MES_NgID { get; set; }
        public string MES_PickingID { get; set; }
        public string MES_ReworkID { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string ReMark { get; set; }

        public int FlowChart_Master_UID { get; set; }
        public string Color { get; set; }
        public string MES_GoodProductID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSyncNG { get; set; }
        public string VM_IsEnabled { get; set; }
        public string VM_IsSyncNG { get; set; }
    }
}
