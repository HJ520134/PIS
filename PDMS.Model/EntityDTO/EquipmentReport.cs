using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class EquipmentReport : EntityDTOBase
    {
        //厂区功能厂对照关系ID
        public int System_FunPlant_UID { get; set; }
        //专案ID
        public int Project_UID { get; set; }
        //专案名称
        public string Project_Name { get; set; }
        //机台类型
        //机台名称
        public string Class_Desc { get; set; }
        //厂商名称
        public string Mfg_Of_Asset { get; set; }
        //OP名称
        public string OP_Name { get; set; }
        //功能厂名称
        public string FunPlant_Name { get; set; }
        //机台总数量
        public int SumALL { get; set; }
        //维修中数量
        public int SumMaintenance { get; set; }
        //待备品数量
        public int SumSpareparts { get; set; }
        //可用数量
        public int SumAvailable { get; set; }
        //OPID
        public int OPType_OrganizationUID { get; set; }
        //专案ID
        public int FunPlant_OrganizationUID { get; set; }
        //功能厂ID 
        public int Plant_OrganizationUID { get; set; }
        //OPID查询时用的
        public int Organization_UID { get; set; }    
        public  string Repair_Result { get; set; }

        //2018-05-03 add by karl APP使用可用率
        public double AvailableRate { get; set; }
        public string ntid { get; set; }
    }
}
