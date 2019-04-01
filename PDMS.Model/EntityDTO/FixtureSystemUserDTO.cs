using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
  public   class FixtureSystemUserDTO : EntityDTOBase
    {
        /// <summary>
        /// 廠區組織流水號
        /// </summary>
        public int? Plant_Organization_UID { get; set; }
        /// <summary>
        /// Business Group流水號
        /// </summary>
        public int? BG_Organization_UID { get; set; }
        /// <summary>
        /// FunPlant_Organization_UID
        /// </summary>
        public int? FunPlant_Organization_UID { get; set; }
       /// <summary>
       /// 当前用户UID
       /// </summary>
        public int Account_UID { get; set; }
        /// <summary>
        /// 当前用户NTID
        /// </summary>
        public string User_NTID { get; set; }
        /// <summary>
        /// 当前用户名称
        /// </summary>
        public string User_Name { get; set; }
        /// <summary>
        /// 当前用户NTID和名称
        /// </summary>
        public string User_NTID_Name { get; set; }
        public bool Enable_Flag { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public int System_Language_UID { get; set; }
        public string LoginToken { get; set; }
        public bool MH_Flag { get; set; }
        public bool IsMulitProject { get; set; }
        public int? flowChartMaster_Uid { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeePassword { get; set; }
        public int EnableEmpIdLogin { get; set; }
        public string Building { get; set; }
    }
}
