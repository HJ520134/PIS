using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class CNCToolInfo
    {
        public List<CNCToolData> Content = new List<CNCToolData>();
    }

    public class CNCToolData
    {
        //刀具编号
        public int ToolNo { get; set; }
        //使用次数
        public int UseCount { get; set; }
        //刀具使用寿命次数
        public int LimitCount { get; set; }
    }
}
