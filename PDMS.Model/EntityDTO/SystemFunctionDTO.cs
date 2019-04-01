using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemFunctionDTO:EntityDTOBase
    {
        public int Function_UID { get; set; }
        //这里面Parent_Function_UID不能设置为int?，不然画面左侧导航会出不来，具体什么原因不清楚，add by rock 2017-08-14
        public int Parent_Function_UID { get; set; }
        public string Function_ID { get; set; }
        public string Function_Name { get; set; }
        public string Function_Desc { get; set; }
        public int Order_Index { get; set; }
        public string Icon_ClassName { get; set; }
        public string URL { get; set; }
        public string Mobile_URL { get; set; }
        public bool Is_Show { get; set; }

        public int System_Language_UID { get; set; }
        public string Table_ColumnName { get; set; }
        public string ResourceValue { get; set; }


    }
}
