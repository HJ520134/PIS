using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class API_MachineStatus
    {
        public int ID { get; set; }
        public string dSn { get; set; }
        public string AuthCode { get; set; }
        public string MachineName { get; set; }
        public string MachineType { get; set; }
        public string PoorNum { get; set; }
        public string TotalNum { get; set; }//;--加工工件数
        public string Status { get; set; }//--机台状态
        public string Category { get; set; }//--报警类型
        public string ErrorCode { get; set; }//--报警号
        public string EventTime { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
