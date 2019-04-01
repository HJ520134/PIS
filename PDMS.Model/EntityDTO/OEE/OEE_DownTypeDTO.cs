using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_DownTypeDTO : BaseModel
    {

        public int OEE_DownTimeType_UID { get; set; }
        public int Plant_Organization_UID { get; set; }

        public int? BG_Organization_UID { get; set; }

        public int? FunPlant_Organization_UID { get; set; }
        public string Type_Name { get; set; }
        public string Type_Code { get; set; }
        public int Sequence { get; set; }
        public bool Is_Enable { get; set; }
        public bool? IsEnabled { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string OP_Organization_Name { get; set; }
        public string FuncPlant_Organization_Name { get; set; }
        public string Modifyer { get; set; }
        //Equip - Pneumatic/Hydraulics 设备-电气 Equip - Roftware 设备-软件 Process-adjustment 工艺-调试 Process-Consumable 工艺-更换耗材 Maintenace 保养 Other 其他
    }

}
