using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class HumanResourcesSummaryDTO
    {
        /// <summary>
        /// 专案名称
        /// </summary>
        public string FlowerName { get; set; }
        /// <summary>
        /// 投入量
        /// </summary>
        public int InputNum { get; set; }
        /// <summary>
        /// 直通率
        /// </summary>
        public decimal OnePass { get; set; }
        /// <summary>
        /// 产出
        /// </summary>
        public int OutputNum { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public string FunPlant { get; set; }
        /// <summary>
        /// 班长当日出勤
        /// </summary>
        public int SquadLeaderNow { get; set; }
        /// <summary>
        /// 班长七轮休
        /// </summary>
        public int SquadLeaderRound { get; set; }
        /// <summary>
        /// 技术员当日出勤
        /// </summary>
        public int TechnicianNow { get; set; }
        /// <summary>
        /// 技术员七轮休
        /// </summary>
        public int TechnicianRound { get; set; }
        /// <summary>
        /// 作业员当日出勤
        /// </summary>
        public int OPNow { get; set; }
        /// <summary>
        /// 作业员七轮休
        /// </summary>
        public int OPRound { get; set; }
        /// <summary>
        /// 物料员当日出勤
        /// </summary>
        public int MaterialKeeperNow { get; set; }
        /// <summary>
        /// 物料员七轮休
        /// </summary>
        public int MaterialKeeperRound { get; set; }
        /// <summary>
        /// 当日出勤总计
        /// </summary>
        public int TotalNow { get; set; }
        /// <summary>
        /// 七轮休总计
        /// </summary>
        public int TotalRound { get; set; }

        public string OtherNow { get; set; }
    }

    public class HumanProjectInfo
    {
        public decimal Estimate_Yield { get; set; }
        public string Project_Name { get; set; }
    }
}
