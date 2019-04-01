using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Machine_Station_CustomerDTO : EntityDTOBase
    {
        public int Machine_Station_UID { get; set; }
        public int Machine_Customer_UID { get; set; }
        public string MES_Station_Name { get; set; }
        public string PIS_Station_Name { get; set; }
        public string MES_Customer_Name { get; set; }
        public string PIS_Customer_Name { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
    }

    /// <summary>
    /// 获取Mes不良明细的接口请求参数
    /// </summary>
    public class MES_NGAPIParam : EntityDTOBase
    {
        public string MES_Customer_Name { get; set; }
        public string PIS_Customer_Name { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class MesNGResult
    {
        public string DefectName { get; set; }
        public int NG_Point { get; set; }
    }

    public class MesNgDetail
    {
        public string SN { get; set; }
        public string DefectName { get; set; }
        public string Station { get; set; }
    }
}
