using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_StationDefectCodeDTO : EntityDTOBase
    {
        public int OEE_StationDefectCode_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// 厂区名字
        /// </summary>
        public string Plant_Organization_Name { get; set; }
        /// <summary>
        /// OP—ID
        /// </summary>
        public int BG_Organization_UID { get; set; }

        /// <summary>
        /// OP-Name
        /// </summary>
        public string BG_Organization_Name { get; set; }
        /// <summary>
        /// 功能厂ID
        /// </summary>
        public Nullable<int> FunPlant_Organization_UID { get; set; }

        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlant_Organization_Name { get; set; }
        /// <summary>
        /// 专案ID
        /// </summary>
        public int Project_UID { get; set; }
        /// <summary>
        /// 专案名称
        /// </summary>
        public string Project_Name { get; set; }

        public string Line_Name { get; set; }

        public string Station_Name { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int Sequence { get; set; }
        public string Defect_Code { get; set; }
        public string DefectEnglishName { get; set; }
        public string DefecChinesetName { get; set; }
        public bool Is_Enable { get; set; }
        public int Modify_UID { get; set; }
        public string Modifyer { get; set; }
        public System.DateTime Modify_Date { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int DefectNum { get; set; }
        public int OEE_DefectCodeDailyNum_UID { get; set; }

        public DateTime currentDate { get; set; }
        public string currentTimeInterval { get; set; }

        public bool? IsEnabled { get; set; }
    }

}
